using Handy.Models;

namespace Handy;

public interface IADService
{
    Task<List<BillutinboardModel>>GetAllRecords();
    Task<BillutinboardModel>GetRecord(int id);
    Task<bool>UploadRecord(BillutinboardModel model, List<IFormFile> files);
}
