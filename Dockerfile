FROM nginx:latest

# copy config
COPY  ./archive/nginx.conf /etc/nginx/conf.d/default.conf

# copy pages
WORKDIR /var/www
COPY ./archive/pages/ /var/www/

# copy normal content
COPY ./StarmixInfo/wwwroot/ /var/www/
COPY ./archive/excludes/site.min.css /var/www/css/
COPY ./archive/excludes/site.min.js /var/www/js/
