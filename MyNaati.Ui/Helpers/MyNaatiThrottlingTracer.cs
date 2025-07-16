using System;
using System.Net.Http;
using System.Web.Http.Tracing;

namespace MyNaati.Ui.Helpers
{
    public class MyNaatiThrottlingTracer : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level,
            Action<TraceRecord> traceAction)
        {
            TraceRecord rec = new TraceRecord(request, category, level);
            traceAction(rec);
            WriteTrace(rec);
        }

        protected void WriteTrace(TraceRecord rec)
        {
            var message = string.Format("{0};{1};{2}",
                rec.Operator, rec.Operation, rec.Message);
            System.Diagnostics.Trace.WriteLine(message, rec.Category);
        }
    }
}