FROM ubuntu:22.04

ARG DEBIAN_FRONTEND=noninteractive
ARG SERVER_BUILD_PATH=Builds/EdgegapServer

COPY ${SERVER_BUILD_PATH} /root/build/

WORKDIR /root/

RUN chmod +x /root/build/ServerBuild

RUN apt-get update && \
    apt-get install -y ca-certificates && \
    apt-get clean && \
    update-ca-certificates

# exec: the server replaces bash, so it gets the stop signal. No `env` dump: it would print every
# injected variable, including secrets and the deployment's self-stop token, to the deployment logs.
CMD ["/bin/bash", "-c", "exec /root/build/ServerBuild -batchmode -nographics $UNITY_COMMANDLINE_ARGS"]
