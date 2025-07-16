{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "naati-infra.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "naati-infra.fullname" -}}
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
{{- define "naati-infra.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "naati-infra.labels" -}}
helm.sh/chart: {{ include "naati-infra.chart" . }}
{{ include "naati-infra.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "naati-infra.selectorLabels" -}}
app.kubernetes.io/name: {{ include "naati-infra.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create the name of the service account to use
*/}}
{{- define "naati-infra.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
    {{ default (include "naati-infra.fullname" .) .Values.serviceAccount.name }}
{{- else -}}
    {{ default "default" .Values.serviceAccount.name }}
{{- end -}}
{{- end -}}


{{/*
Create the name of the key vault secret 
*/}}
{{- define "naati-infra.keyVaultSecret" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName "keyvault" | trunc 63 | trimSuffix "-" }}
{{- end -}}

{{/*
Create the name of the key persistent storage
*/}}
{{- define "naati-infra.persistentStorage" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName  "persistent-storage" | trunc 63 | trimSuffix "-" }}
{{- end -}}


{{/*
Create the name of the key temp persistent storage
*/}}
{{- define "naati-infra.tempPersistentStorage" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName  "temp-persistent-storage" | trunc 63 | trimSuffix "-" }}
{{- end -}}

{{/*
Create the name of the key persistent volume claim 
*/}}
{{- define "naati-infra.volumeClaimName" -}}
{{- printf "%s-%s-%s"  (default "default-volume-claim"  .Values.targetRelease) .Values.projectName  "volume-claim" | trunc 63 | trimSuffix "-" }}
{{- end -}}


{{/*
Create the name of the key temp persistent volume claim 
*/}}
{{- define "naati-infra.tempVolumeClaimName" -}}
{{- printf "%s-%s-%s"  (default "default-volume-claim"  .Values.targetRelease) .Values.projectName  "temp-volume-claim" | trunc 63 | trimSuffix "-" }}
{{- end -}}

{{/*
Create the name of the storage secret
*/}}
{{- define "naati-infra.storageSecret" -}}
{{- printf "%s-%s-%s"  (default "default-storage-secret"  .Values.targetRelease) .Values.projectName  "storage" | trunc 63 | trimSuffix "-"  }}
{{- end -}}

{{/*
Create the name of the temp storage secret
*/}}
{{- define "naati-infra.tempStorageSecret" -}}
{{- printf "%s-%s-%s"  (default "default-temp-storage-secret"  .Values.targetRelease) .Values.projectName  "temp-storage" | trunc 63 | trimSuffix "-"  }}
{{- end -}}


{{/*
Create the name of the storage share name
*/}}
{{- define "naati-infra.storageShareName" -}}
{{- (printf "%s-%s-%s"  (default "default-shareName"  .Values.targetRelease) .Values.projectName  "share") | lower | trunc 63 | trimSuffix "-" }}
{{- end -}}

{{/*
Create the name of the machine key secret
*/}}
{{- define "naati-infra.machineKeySecret" -}}
{{- printf "%s-%s-%s"  (default "default-machinekey-secret"  .Values.targetRelease) .Values.projectName  "machinekey" | trunc 63 | trimSuffix "-"  }}
{{- end -}}


{{- define "naati-infra.ncmsName" -}}
{{- $name := default "ncms" .Values.ncmsFullName -}}
{{- printf "%s-%s" .Values.targetRelease $name | trunc 63 | trimSuffix "-" | quote -}}
{{- end -}}

{{- define "naati-infra.mynaatiName" -}}
{{- $name := default "mynaati" .Values.myNaatiFullName -}}
{{- printf "%s-%s" .Values.targetRelease $name | trunc 63 | trimSuffix "-" | quote -}}
{{- end -}}

{{/*
  Create the name of the key limit range pods
*/}}
{{- define "naati-infra.limitRangePods" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName  "pods-limit-range" | trunc 63 | trimSuffix "-" }}
{{- end -}}
 
 {{/*
Create the name of the key limit range persistent volume claims
*/}}
{{- define "naati-infra.limitPvc" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName  "pvc-limit-range" | trunc 63 | trimSuffix "-" }}
{{- end -}} 

{{/*
Create the name of the resource quota for namespaces
*/}}
{{- define "naati-infra.resourceQuotaRangeMemCpuPvc" -}}
{{- printf "%s-%s-%s" (default "default" .Values.targetRelease) .Values.projectName  "resource-quota" | trunc 63 | trimSuffix "-" }}
{{- end -}}