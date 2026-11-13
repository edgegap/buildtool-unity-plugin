using System.Collections.Generic;
using Newtonsoft.Json;

namespace Edgegap.Editor.Api.Models.Results
{
    /// <summary>
    /// Result model for `GET v1/container-registry/images/{image_name}/tags`.
    /// </summary>
    public class GetRegistryImageTagsResult
    {
        [JsonProperty("data")]
        public List<RegistryImageTag> Data { get; set; }

        [JsonProperty("pagination")]
        public RegistryImageTagsPagination Pagination { get; set; }
    }

    public class RegistryImageTag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }

    public class RegistryImageTagsPagination
    {
        [JsonProperty("has_next")]
        public bool HasNext { get; set; }
    }
}
