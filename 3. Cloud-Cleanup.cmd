kubectl delete -f Pipeline/Infrastructure/deploy-zookeeper-1.yml
kubectl delete -f Pipeline/Infrastructure/deploy-zookeeper-2.yml
kubectl delete -f Pipeline/Infrastructure/deploy-zookeeper-3.yml
kubectl delete -f Pipeline/Infrastructure/deploy-kafka-1.yml
kubectl delete -f Pipeline/Infrastructure/deploy-kafka-2.yml
kubectl delete -f Pipeline/Infrastructure/deploy-kafka-3.yml
kubectl delete -f Pipeline/Infrastructure/deploy-schema-registry.yml
kubectl delete -f Pipeline/Infrastructure/deploy-kowl.yml
kubectl delete -f Pipeline/Infrastructure/deploy-redis.yml
kubectl delete -f Pipeline/Infrastructure/deploy-mongodb.yml

kubectl delete -f Pipeline/Services/deploy-input-service.yml
kubectl delete -f Pipeline/Services/deploy-collision-service.yml
kubectl delete -f Pipeline/Services/deploy-world-service.yml
kubectl delete -f Pipeline/Services/deploy-projectile-service.yml
kubectl delete -f Pipeline/Services/deploy-ai-service.yml
