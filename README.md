# 10-Master
Master Thesis Project

--------------------------------------------------

## Local Docker Run With Local Images
Install Docker Desktop

Setup environment:
```
cd /
docker-compose build --no-cache
docker-compose up
```
run the game client

--------------------------------------------------

## Local Docker Run With External Images
Install Docker Desktop

Setup environment:
```
cd /
docker-compose -f docker-compose-github.yml up
```
run the game client

--------------------------------------------------

## Local Kubernetes Run With External Images (Deprecated)
Install Docker Desktop + Kind + Helm

Setup environment:
```
cd /
kind create cluster
```

Setup the infrastructure services:
```
kubectl apply -f Pipeline/Infrastructure/deploy-zookeeper.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kafka.yml
kubectl apply -f Pipeline/Infrastructure/deploy-schema-registry.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kowl.yml
kubectl apply -f Pipeline/Infrastructure/deploy-redis.yml
kubectl apply -f Pipeline/Infrastructure/deploy-mongodb.yml
```

Setup the game services:
```
kubectl apply -f Pipeline/Services/deploy-input-service.yml
kubectl apply -f Pipeline/Services/deploy-collision-service.yml
kubectl apply -f Pipeline/Services/deploy-world-service.yml
kubectl apply -f Pipeline/Services/deploy-projectile-service.yml
kubectl apply -f Pipeline/Services/deploy-ai-service.yml
```

Port forward Kafka for local client:
```
kubectl port-forward services/kowl-service 8080:8080
kubectl port-forward services/kafka-service 19092:19092
kubectl port-forward services/schema-registry-service 8081:8081
kubectl port-forward services/redis-stack 8001:8001
kubectl port-forward services/redis-stack 6379:6379
```

run the game client

Cleanup the cluster:
```
kind delete cluster
```

--------------------------------------------------

## GKE With External Images

Setup the infrastructure services:
```
run "1. Cloud-Setup.cmd"
```

Setup the game services:
```
git commit to Main and let the CI/CD pipeline do it
```

Setup sharding in the database:
```
kubectl exec -it <mongos-pod-name> -- /bin/bash
mongosh -u root -p password
sh.enableSharding("TidesOfPower")
sh.shardCollection("TidesOfPower.Players", { "location.x" : 1, "location.y" : 1} )
sh.status()
use TidesOfPower
db.Players.find()
db.Players.getShardDistribution()
```

Setup KEDA:
```
run "2. Cloud-Scale-Auto.cmd"
```

run the game client

Cleanup the cluster:
```
run "3. Cloud-Cleanup.cmd"
```

--------------------------------------------------

### Extra
Setup sharded MongoDB via helm:
```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install mongodb-service bitnami/mongodb-sharded -f Pipeline/Infrastructure/mongodb-values.yml
```
