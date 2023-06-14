# k8s-demo-app
k8s demo app

## Основы

Серия вводных статей по k8s для ASP.NET
- [Series: Deploying ASP.NET Core applications to Kubernetes](https://andrewlock.net/series/deploying-asp-net-core-applications-to-kubernetes/)


## Собираем кубер кластер на локальном компьютере

### 1 Включаем k8s в Docker Desktop

### 2 Устанавливаем kuber-dashboard

https://github.com/kubernetes/dashboard

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/master/charts/recommended.yaml
```

Накатываем yaml файлы из каталога репозитория \k8s\dashboard

```
kubectl apply -f service-account.yaml
kubectl apply -f cluster-role-binding.yaml
```

Запускаем прокси

```
kubectl proxy
```
Генерим токен

```
kubectl -n kubernetes-dashboard create token admin
```
Открываем урл и логинимся в дашборд

http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/

### 3 Устанавливаем helm

- Скачиваем бинарник https://github.com/helm/helm/releases/
- Прописываем в PATH путь к бинарнику

### 4 Устанавливаем kube-prometeus stack

- [kube-prometheus-stack](https://github.com/prometheus-community/helm-charts/tree/main/charts/kube-prometheus-stack)
- [Kube-Prometheus-Stack installation and configuration](https://www.virtualizationhowto.com/2023/03/kube-prometheus-stack-installation-and-configuration/)
- [Пошаговая настройка Kube-Prometheus-Stack для мониторинга Kubernetes](https://inostudio.com/blog/articles-devops/nastroyka-kube-prometheus-stack/)

```
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update 
```

Для первого запуска:

```
kubectl create namespace monitoring
```
Далее

```
helm install kube-prometheus-demo prometheus-community/kube-prometheus-stack -n monitoring --set prometheus-node-exporter.hostRootFsMount.enabled=false
```
Проверить, что [node-exporter](https://github.com/prometheus/node_exporter) стартовал - https://github.com/prometheus-community/helm-charts/issues/325

### 5 Настраиваем метрики в прометеусе через Prometheus Operator

- [A Beginner's Guide to Using the Prometheus Operator](https://blog.container-solutions.com/prometheus-operator-beginners-guide)

#### Накатываем yaml файлы из каталога репозитория \k8s\prometheus

```
kubectl apply -f service-monitor.yaml -n monitoring
```

### Проверяем доступность таргетов

```
kubectl port-forward -n monitoring svc/prometheus-operated 9090
```

- http://localhost:9090/targets
- http://localhost:9090/targets?search=&scrapePool=serviceMonitor%2Fmonitoring%2Fkube-prometheus-demo-kuber-demo-app%2F0
- http://localhost:9090/graph?g0.expr=aspnetcore_healthcheck_status&g0.tab=1&g0.stacked=1&g0.show_exemplars=0&g0.range_input=15m


#### 6 Устанавливаем плагины графаны

```
kubectl port-forward -n monitoring svc/kube-prometheus-demo-grafana 3000:80
```

- http://localhost:3000/?orgId=1

admin
prom-operator

Дашборды k8s
- [Kubernetes Cluster Prometheus](https://grafana.com/grafana/dashboards/6417-kubernetes-cluster-prometheus/
- [Kubernetes Cluster](https://grafana.com/grafana/dashboards/7249-kubernetes-cluster/)
- [Kubernetes Pod Metrics](https://grafana.com/grafana/dashboards/747-pod-metrics/)

Дашборды asp.net
- [ASP.NET Core Grafana dashboards](https://github.com/JamesNK/aspnetcore-grafana)
- [prometheus-net dashboards](https://github.com/prometheus-net/grafana-dashboards)

### 7 Устанавливаем nginx ингресс (опционально)
см. папку k8s\nginx

## Собираем и деплоим тестовое приложение

### Собираем докер образ солюшена командой 

```
docker build -t kuberdemo:v0.0.12 .
```

### Выкладываем в кубер

Создаем неймспейс
```
kubectl create namespace local
```
### Проверяем чарты из каталога репозитория \k8s\charts

- values.yaml - проставить тег релиза из собранного докеримиджа - tag: "v0.0.12"
- chart.yaml - увеличить версии на +1

### Катим деплой через хелм чарт

Проверка
```
helm upgrade --install kuber-demo-app . --namespace=local --debug --dry-run
```
Выкладка

```
helm upgrade --install kuber-demo-app . --namespace=local --debug 
```

### Проверяем доступность подов и форвардим порты

```
kubectl get pods -n local
```
Форвардим порты

```
kubectl port-forward -n local svc/kuber-demo-app 80
```

### Приложение доступно по локалхосту

- http://localhost/WeatherForecast
- http://localhost/Metrics
- http://localhost/ready
