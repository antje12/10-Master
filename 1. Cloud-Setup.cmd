kubectl apply -f Pipeline/Infrastructure/deploy-zookeeper-1.yml
kubectl apply -f Pipeline/Infrastructure/deploy-zookeeper-2.yml
kubectl apply -f Pipeline/Infrastructure/deploy-zookeeper-3.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kafka-1.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kafka-2.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kafka-3.yml
kubectl apply -f Pipeline/Infrastructure/deploy-schema-registry.yml
kubectl apply -f Pipeline/Infrastructure/deploy-kowl.yml
kubectl apply -f Pipeline/Infrastructure/deploy-redis.yml