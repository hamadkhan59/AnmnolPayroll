using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ITransportRepository : IDisposable
    {
        int AddTransportStop(TransportStop branch);
        TransportStop GetTransportStopById(int messageId);
        TransportStop GetTransportStopByName(string name);
        List<TransportStop> GetAllTransportStops();
        void UpdateTransportStop(TransportStop bracnh);
        void DeleteTransportStop(TransportStop bracnh);

        int AddTransportDriver(TransportDriver branch);
        TransportDriver GetTransportDriverById(int messageId);
        List<TransportDriver> GetAllTransportDrivers();
        void UpdateTransportDriver(TransportDriver bracnh);
        void DeleteTransportDriver(TransportDriver bracnh);

        int AddDriverStop(TransportDriverStop driverStop);
        TransportDriverStop GetDriverStopById(int driverStopId);
        TransportDriverStop GetDriverStopByIdAndStopId(int driverStopId, int stopId);
        TransportDriverStop GetDriverStopByDriverId(int driverStopId);
        List<TransportDriverStop> GetAllTransportDriverStop();
        List<TransportDriverStopModel> GetAllTransportDriverStopModel();
        void DeleteTransportDriverStop(TransportDriverStop driverStop);
    }
}
