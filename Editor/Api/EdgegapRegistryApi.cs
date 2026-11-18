using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Edgegap.Editor.Api.Models.Results;

namespace Edgegap.Editor.Api
{
    /// <summary>
    /// Wraps the v1/container-registry API endpoints: Edgegap Container Registry.
    /// </summary>
    public class EdgegapRegistryApi : EdgegapApiBase
    {
        private const int TagsPageSize = 100;
        private const int MaxTagPages = 100;

        public EdgegapRegistryApi(
            ApiEnvironment apiEnvironment,
            string apiToken,
            EdgegapWindowMetadata.LogLevel logLevel = EdgegapWindowMetadata.LogLevel.Error
        )
            : base(apiEnvironment, apiToken, logLevel) { }

        #region API Methods
        /// <summary>
        /// GET to v1/container-registry/images/{imageName}/tags
        /// - Lists one page of the tags pushed for an image in the Edgegap registry.
        /// </summary>
        /// <param name="imageName">Image name only, without registry host or project.</param>
        /// <returns>
        /// Http info with GetRegistryImageTagsResult data model
        /// - Success: 200
        /// - Fail: 404 (nothing pushed under this image name yet)
        /// </returns>
        public async Task<EdgegapHttpResult<GetRegistryImageTagsResult>> GetImageTags(
            string imageName,
            int page = 1,
            int limit = TagsPageSize
        )
        {
            HttpResponseMessage response = await GetAsync(
                $"v1/container-registry/images/{Uri.EscapeDataString(imageName)}/tags?page={page}&limit={limit}"
            );
            EdgegapHttpResult<GetRegistryImageTagsResult> result =
                new EdgegapHttpResult<GetRegistryImageTagsResult>(response); // MIRROR CHANGE: 'new()' not supported in Unity 2020

            return result;
        }

        /// <summary>
        /// Checks if a tag was already pushed for an image in the Edgegap registry.
        /// </summary>
        /// <param name="imageName">Image name only, without registry host or project.</param>
        /// <param name="tag">Tag to look for.</param>
        /// <returns>true if the tag exists; false if the image or the tag doesn't exist yet.</returns>
        /// <exception cref="Exception">The registry couldn't be read.</exception>
        public async Task<bool> ImageTagExists(string imageName, string tag)
        {
            for (int page = 1; page <= MaxTagPages; page++)
            {
                EdgegapHttpResult<GetRegistryImageTagsResult> result = await GetImageTags(
                    imageName,
                    page
                );

                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    // nothing pushed under this image name yet
                    return false;
                }

                if (!result.IsResultCode200)
                {
                    throw new Exception(
                        $"Unable to list existing tags of image '{imageName}' in registry"
                            + $" (Error {(int)result.StatusCode}: {result.ReasonPhrase})."
                    );
                }

                if (result.Data?.Data != null)
                {
                    foreach (RegistryImageTag imageTag in result.Data.Data)
                    {
                        if (string.Equals(imageTag.Tag, tag, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }

                if (result.Data?.Pagination == null || !result.Data.Pagination.HasNext)
                {
                    return false;
                }
            }

            return false;
        }
        #endregion // API Methods
    }
}
