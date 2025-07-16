{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "mynaati.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "mynaati.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "mynaati.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "mynaati.labels" -}}
helm.sh/chart: {{ include "mynaati.chart" . }}
{{ include "mynaati.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "mynaati.selectorLabels" -}}
app.kubernetes.io/name: {{ include "mynaati.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "mynaati.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "mynaati.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}

{{/*
Create the name of the key vault secret 
*/}}
{{- define "mynaati.keyVaultSecret" -}}
{{- printf "%s-%s-%s" (default "default" .Release.Name) .Values.sharedResourcesName "keyvault" | trunc 63 | trimSuffix "-" }}
{{- end -}}

{{/*
Create the name of the key persistent storage
*/}}
{{- define "mynaati.persistentStorage" -}}
{{- printf "%s-%s-%s" (default "default" .Release.Name) .Values.sharedResourcesName "persistent-storage" | trunc 63 | trimSuffix "-"  }}
{{- end -}}

{{/*
Create the name of the key temp persistent storage
*/}}
{{- define "mynaati.tempPersistentStorage" -}}
{{- printf "%s-%s-%s" (default "default" .Release.Name) .Values.sharedResourcesName "temp-persistent-storage" | trunc 63 | trimSuffix "-"  }}
{{- end -}}

{{/*
Create the name of the key persistent volume claim 
*/}}
{{- define "mynaati.volumeClaimName" -}}
{{- printf "%s-%s-%s"  (default "default-volume-claim"  .Release.Name) .Values.sharedResourcesName "volume-claim" | trunc 63 | trimSuffix "-"  }}
{{- end -}}


{{/*
Create the name of the key temp persistent volume claim 
*/}}
{{- define "mynaati.tempVolumeClaimName" -}}
{{- printf "%s-%s-%s"  (default "default-volume-claim"  .Release.Name) .Values.sharedResourcesName "temp-volume-claim" | trunc 63 | trimSuffix "-"  }}
{{- end -}}


{{- define "mynaati.ncmsName" -}}
{{- $name := default "ncms" .Values.ncmsFullName -}}
{{- printf "%s%s-%s" "https://" .Release.Name $name | trunc 63 | trimSuffix "-" | quote -}}
{{- end -}}

{{/*
Create the name of the machine key secret
*/}}
{{- define "mynaati.machineKeySecret" -}}
{{- printf "%s-%s-%s"  (default "default-machinekey-secret"  .Release.Name) .Values.sharedResourcesName  "machinekey" | trunc 63 | trimSuffix "-"  }}
{{- end -}}

{{/*
Create the emil securepay paymentUrl
*/}}
{{- define "mynaati.securepayPaymentUrl" -}}
{{- printf "https://test.securepay.com.au/xmlapi/payment"  | trunc 63 | trimSuffix "-"  }}
{{- end -}}


