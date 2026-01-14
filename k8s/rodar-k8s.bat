kubectl apply -f ./rabbitmq.yaml
kubectl apply -f ./payments-sqlserver.yaml
kubectl apply -f ./payments-configmap.yaml
kubectl apply -f ./payments-secret.yaml
kubectl apply -f ./payments-api.yaml
kubectl apply -f ./payments-service.yaml
kubectl apply -f ./payments-hpa.yaml
minikube service payments-api
