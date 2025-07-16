{{/*
  Create the name of the key limit range pods
*/}}
{{- define "cluster-infra.limitRangePods" -}}
{{- printf "%s-%s" (default "default" .Release.Name) "pods-limit-range" | trunc 63 | trimSuffix "-" }}
{{- end -}}