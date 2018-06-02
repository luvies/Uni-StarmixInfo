# used for building the archive docker container
IMAGE=starmixinfo-archive-img
CONTAINER=ws_starmixinfo_archive

.PHONY: build
build:
	docker build . -t $(IMAGE)

run:
	docker container run \
		--name $(CONTAINER) \
		--network nw_starmixinfo \
		--restart always \
		-d $(IMAGE):latest
	
stop:
	docker container stop $(CONTAINER)
	docker container rm $(CONTAINER)

test:
	docker run --name $(CONTAINER) -p 80:80 -d --rm $(IMAGE)

test-logs:
	docker logs $(CONTAINER)

test-inspect:
	docker exec -it $(CONTAINER) /bin/sh

test-rm:
	docker rm $(CONTAINER)

test-stop:
	docker stop $(CONTAINER)

clean:
	docker rmi -f $(IMAGE)
