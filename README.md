# iambetter

## Docker setup

Use the following command to add pull mongodb image and start container locally to the port 27017. 

```
docker run -d --name mongodb -p 27017:27017 mongo:latest --bind_ip_all

```


_Please note: mongo db community server was not working on windows machine_ 
