FROM golang:1.19-alpine AS base
RUN apk add curl \
    && mkdir /app
WORKDIR /app

FROM golang:1.19-alpine AS build
RUN mkdir /src
ADD . /src
WORKDIR /src
RUN go mod init main
RUN go mod tidy

FROM build AS publish
RUN go build -o main .

FROM base AS final
WORKDIR /app
COPY --from=publish /src/* ./

CMD ["/app/main"]