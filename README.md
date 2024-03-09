# 10-Master
Master Thesis Project



## Local Docker Run With Local Images
Install Docker Desktop

Setup environment:
```
cd /
docker compose build --no-cache
docker compose up
```
run the game client



## Local Docker Run With External Images
Install Docker Desktop

Setup environment:
```
cd /
docker-compose -f docker-compose-github.yml up
```
run the game client



## Local Kubernetes Run With External Images
Install Docker Desktop + Kind + Helm

Setup environment:
```
cd /
kind create cluster
```

Setup the infrastructure services:
```
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-zookeeper.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-kafka.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-schema-registry.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-kowl.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-mongodb.yml
```

Setup the game services:
```
kubectl apply -f Pipeline/Kubernetes/Services/deploy-input-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-collision-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-world-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-tick-service.yml
```

Setup sharding in the database:
```
kubectl port-forward services/mongodb-service 27017:27017
mongosh -u root -p password
sh.status()
sh.enableSharding("TidesOfPower")
sh.shardCollection("TidesOfPower.Entities", { "location.x" : 1, "location.y" : 1} )
```

Port forward Kafka for local client:
```
kubectl port-forward services/kowl-service 8080:8080
kubectl port-forward services/kafka-service 19092:19092
kubectl port-forward services/schema-registry-service 8081:8081
```

run the game client

Cleanup the cluster:
```
kind delete cluster
```

### Extra
Setup sharded MongoDB via helm:
```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install mongodb-sharded bitnami/mongodb-sharded -f Pipeline/Kubernetes/Helm/mongodb-values.yml
```

Setup distributed Kafka via helm:

~~helm install kafka-service bitnami/kafka -f Pipeline\Kubernetes\Helm\kafka-values.yml~~

~~helm install schema-registry-service bitnami/schema-registry -f Pipeline\Kubernetes\Helm\schema-registry-values.yml~~
