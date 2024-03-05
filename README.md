# 10-Master
Master Thesis Project

# Local Docker Run With Local Images
Install Docker Desktop
```cd /```
```docker compose build --no-cache```
```docker compose up```

run the game client

# Local Kubernetes Run With Local Images
Install Docker Desktop + Kind + Helm
```cd /```
```docker compose build --no-cache```
```kind create cluster```

```
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install mongodb-sharded bitnami/mongodb-sharded -f Pipeline/Kubernetes/Helm/mongodb-values.yml

helm install kafka bitnami/kafka -f Pipeline\Kubernetes\Helm\kafka-values.yml
helm install schema-registry bitnami/schema-registry -f Pipeline\Kubernetes\Helm\schema-registry-values.yml
```

```
kubectl port-forward services/my-mongodb-sharded 27017:27017
mongosh -u root -p password
sh.status()
sh.enableSharding("TidesOfPower")
sh.shardCollection("TidesOfPower.Entities", { "location.x" : 1, "location.y" : 1} )
```

~~kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-mongodb.yml~~

```
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-zookeeper.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-kafka.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-schema-registry.yml
kubectl apply -f Pipeline/Kubernetes/Infrastructure/deploy-kowl.yml
```

```
kind load docker-image 10-master-input-service --name kind
kind load docker-image 10-master-collision-service --name kind
kind load docker-image 10-master-world-service --name kind
kind load docker-image 10-master-tick-service --name kind
```

```
kubectl apply -f Pipeline/Kubernetes/Services/deploy-input-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-collision-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-world-service.yml
kubectl apply -f Pipeline/Kubernetes/Services/deploy-tick-service.yml
```

```
kubectl port-forward services/kowl-service 8080:8080
kubectl port-forward services/kafka 9092:9092
kubectl port-forward services/schema-registry 8081:8081
```

run the game client

```kind delete cluster```
