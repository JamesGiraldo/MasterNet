using System.Globalization;
using CsvHelper;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Common;

namespace MasterNet.Infrastructure.Reports;

public class ReportService<T> : IReportService<T> where T : BaseEntity
{
    public async Task<MemoryStream> GetCsvReport(List<T> records)
    {
        using var memoryStream = new MemoryStream();
        using var textWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture);

        await csvWriter.WriteRecordsAsync(records);
        textWriter.Flush();
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}