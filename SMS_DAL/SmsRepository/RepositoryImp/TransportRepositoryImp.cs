using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class TransportRepositoryImp : ITransportRepository
    {
        private SC_WEBEntities2 dbContext1;

        public TransportRepositoryImp(SC_WEBEntities2 context)   
        {  
            dbContext1 = context;  
        }

        SC_WEBEntities2 dbContext
        {
            get
            {
                if (dbContext1 == null || this.disposed == true)
                    dbContext1 = new SC_WEBEntities2();
                this.disposed = false;
                return dbContext1;
            }
        }

        public int AddTransportStop(TransportStop branch)
        {
            int result = -1;
            if (branch != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.TransportStops.Add(branch);
                dbContext.SaveChanges();
                result = branch.Id;
            }

            return result;
        }

        public TransportStop GetTransportStopById(int messageId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.TransportStops.Where(x => x.Id == messageId).FirstOrDefault();
        }

        public TransportStop GetTransportStopByName(string name)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.TransportStops.Where(x => x.Name == name).FirstOrDefault();
        }

        public List<TransportStop> GetAllTransportStops()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.TransportStops.ToList();
        }

        public void UpdateTransportStop(TransportStop bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(bracnh).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteTransportStop(TransportStop bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.TransportStops.Remove(bracnh);
                dbContext.SaveChanges();
            }
        }

        public int AddTransportDriver(TransportDriver branch)
        {
            int result = -1;
            if (branch != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.TransportDrivers.Add(branch);
                dbContext.SaveChanges();
                result = branch.Id;
            }

            return result;
        }

        public TransportDriver GetTransportDriverById(int messageId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.TransportDrivers.Where(x => x.Id == messageId).FirstOrDefault();
        }

        public List<TransportDriver> GetAllTransportDrivers()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.TransportDrivers.ToList();
        }

        public void UpdateTransportDriver(TransportDriver bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(bracnh).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteTransportDriver(TransportDriver bracnh)
        {
            if (bracnh != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.TransportDrivers.Remove(bracnh);
                dbContext.SaveChanges();
            }
        }

        public int AddDriverStop(TransportDriverStop driverStop)
        {
            int result = -1;
            if (driverStop != null)
            {
                dbContext.TransportDriverStops.Add(driverStop);
                dbContext.SaveChanges();
                result = driverStop.Id;
            }

            return result;
        }

        public TransportDriverStop GetDriverStopById(int driverStopId)
        {
            return dbContext.TransportDriverStops.Where(x => x.Id == driverStopId).FirstOrDefault();
        }

        public TransportDriverStop GetDriverStopByIdAndStopId(int driverStopId, int stopId)
        {
            return dbContext.TransportDriverStops.Where(x => x.DriverId == driverStopId && x.StopId == stopId).FirstOrDefault();
        }

        public TransportDriverStop GetDriverStopByDriverId(int driverStopId)
        {
            return dbContext.TransportDriverStops.Where(x => x.DriverId == driverStopId).FirstOrDefault();
        }

        public List<TransportDriverStop> GetAllTransportDriverStop()
        {
            return dbContext.TransportDriverStops.ToList();
        }

        public List<TransportDriverStopModel> GetAllTransportDriverStopModel()
        {
            var query = from driverStop in dbContext.TransportDriverStops
                        join stop in dbContext.TransportStops on driverStop.StopId equals stop.Id
                        select new TransportDriverStopModel
                        {
                            Id = driverStop.Id,
                            DriverId = driverStop.DriverId,
                            StopId = driverStop.StopId,
                            StopName = stop.Name,
                            StopRent = (int) (stop.Rent == null ? 0 : stop.Rent)
                        };
            return query.ToList();
        }


        public void DeleteTransportDriverStop(TransportDriverStop clas)
        {
            if (clas != null)
            {
                dbContext.TransportDriverStops.Remove(clas);
                dbContext.SaveChanges();
            }
        }



        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }  
    }
}
