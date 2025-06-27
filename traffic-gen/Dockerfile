FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ARG TARGETPLATFORM
ARG BUILDPLATFORM
RUN echo "I am building on $BUILDPLATFORM for $TARGETPLATFORM"
ENV DOTNET_EnableWriteXorExecute=0
WORKDIR /App

COPY . /App

RUN dotnet restore

RUN dotnet publish -c Release -o out


# Build runtime image, using multi-stage builds
# https://docs.docker.com/build/building/multi-stage/

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /App

COPY --from=build-env /App/out .

ENTRYPOINT [ "dotnet", "traffic-gen.dll" ]







