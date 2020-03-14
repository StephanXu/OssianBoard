FROM node:13.7 AS build
WORKDIR /usr/src/app
COPY ["package.json", "package-lock.json*", "npm-shrinkwrap.json*", "./"]
RUN npm install --silent --registry=https://registry.npm.taobao.org
COPY ./ .
RUN npm run proto
RUN npm run build

FROM nginx:1.17.8-alpine AS final
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /usr/src/app/dist /usr/share/nginx/html