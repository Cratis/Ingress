# syntax=docker/dockerfile:1

####################################
# Cratis Ingress
# Build runtime image.
####################################
FROM mcr.microsoft.com/dotnet/runtime:10.0-noble
ARG VERSION

RUN echo Version = ${VERSION}

# Install Tools
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Add Tini to get a real init process
RUN ARCH=$(dpkg --print-architecture) && \
    curl -LJ https://github.com/krallin/tini/releases/download/v0.19.0/tini-${ARCH} --output /usr/bin/tini && \
    chmod +x /usr/bin/tini

WORKDIR /app

COPY ./Source/Ingress/out ./

ENTRYPOINT ["/usr/bin/tini", "--", "dotnet", "/app/Ingress.dll"]
