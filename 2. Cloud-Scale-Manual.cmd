kubectl scale deployment input-service --replicas=10
kubectl scale deployment collision-service --replicas=10
kubectl scale deployment world-service --replicas=10
kubectl scale deployment projectile-service --replicas=10
kubectl scale deployment ai-service --replicas=10
kubectl delete pods --all
