To choose install the chart to an enviroment you have to use the command bellow:

helm install [chart] . -f values.[environment].yaml

eg:
helm install naati . -f values.dev.yaml