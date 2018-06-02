FROM nginx:latest

# copy config
COPY  ./static/nginx.conf /etc/nginx/conf.d/default.conf

# copy pages
WORKDIR /var/www
COPY ./static/pages/ /var/www/

# copy normal content
COPY ./StarmixInfo/wwwroot/ /var/www/
