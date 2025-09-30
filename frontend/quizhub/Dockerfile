FROM node:20-alpine AS build
WORKDIR /app

COPY frontend/quizhub/package*.json ./
RUN npm ci

COPY frontend/quizhub ./

ARG REACT_APP_API_BASE=http://restfulapi.quiz:8080
ENV REACT_APP_API_BASE=${REACT_APP_API_BASE}

RUN npm run build

FROM nginx:alpine
COPY docker/nginx/frontend.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
