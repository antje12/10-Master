helm repo add kedacore https://kedacore.github.io/charts
helm repo update
helm install keda kedacore/keda
kubectl apply -f Pipeline/Scaling/deploy-keda-scalers.yml
