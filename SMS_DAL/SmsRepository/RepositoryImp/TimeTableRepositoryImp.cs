using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class TimeTableRepositoryImp : ITimeTableRepository
    {
        private readonly SC_WEBEntities2 dbContext;

        public TimeTableRepositoryImp(SC_WEBEntities2 context)
        {
            //dbContext.Configuration.LazyLoadingEnabled = false;            
            dbContext = context;
        }

        #region

        public List<TimeTable> GetByBranch(int brachId)
        {
            return dbContext.TimeTables.Where(n => n.BranchId == brachId).OrderBy(n => n.Day).ThenBy(n => n.Slot).ThenBy(n => n.ClassSectionId).ThenBy(n => n.TeacherId).ToList();
        }

        public ScheduleViewModel GetViewModelByBranch(int branchId)
        {
            var timeTable = dbContext.TimeTables.Where(n => n.BranchId == branchId).ToList();
            var scheduleViewModel = new ScheduleViewModel();
            //scheduleViewModel.ClassSectionLectures = new List<ClassSectionLectures>();
            //scheduleViewModel.LectureSlots = new List<LectureSlot>();
            scheduleViewModel.ClassSectionLectures = timeTable.GroupBy(n => n.ClassSectionId).Select(n => new ClassSectionLectures()
            {
                ClassSectionId = n.Key,
                Lectures = n.Select(s => new Lecture()
                {
                    TimeTableId = s.ID,
                    SlotId = s.Slot,
                    ClassSectionId = s.ClassSectionId,
                    ClassSectionName = s.ClassSection.Class.Name + " " + s.ClassSection.Section.Name,
                    TeacherId = s.TeacherId,
                    TeacherName = s.Staff.Name,
                    SubjectId = s.ClassSubjectId,
                    SubjectName = s.RegisterCourse.Subject.Name,
                    RoomId = s.RoomId
                }).ToList()
            }).ToList();

            foreach (var v in scheduleViewModel.ClassSectionLectures)
            {
                v.ClassSectionName = v.Lectures.Count() > 0 ? v.Lectures.First().ClassSectionName : null;
            }

            scheduleViewModel.LectureSlots = timeTable.GroupBy(n => n.Slot).Select(n => new LectureSlot()
            {
                SlotId = n.Key,
                TeacherIds = n.Select(s => s.TeacherId).ToList(),
                ClassSectionIds = n.Select(s => s.ClassSectionId).ToList(),
                TimeTableIds = n.Select(s => s.ID).ToList()
            }).OrderBy(n => n.SlotId).ToList();

            return scheduleViewModel;
        }

        public bool SaveChanges(int branchId, List<LectureSlot> lectureSlots)
        {
            var timeTable = dbContext.TimeTables.Where(n => n.BranchId == branchId).ToList();
            foreach (var slot in lectureSlots)
            {
                var itemsBySlot = timeTable.Where(n => slot.TimeTableIds.Contains(n.ID));
                foreach (var item in itemsBySlot)
                {
                    item.Slot = slot.SlotId;
                    dbContext.Entry(item).State = EntityState.Modified;
                }
            }
            dbContext.SaveChanges();

            return RemoveRoomConflicts(branchId);
        }
        public bool CreateOrUpdate(int branchId, List<TimeTable> objs)
        {
            if (objs.Count() > 0)
            {
                var existing = dbContext.TimeTables.Where(n => n.BranchId == branchId).ToList();
               foreach (var e in existing)
               {
                   dbContext.TimeTables.Remove(e);
               }
            }

            foreach (var obj in objs)
            {
                if (obj.ID > 0)
                {
                    dbContext.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    dbContext.TimeTables.Add(obj);
                }
            }
            dbContext.SaveChanges();
            return true;
        }

        public bool RemoveRoomConflicts(int branchId)
        {
            var timeTables = dbContext.TimeTables.Where(n => n.BranchId == branchId).ToList();
            var orderedSlots = timeTables.GroupBy(n => n.Slot).Select(n => new { slot = n.Key, count = n.Count() }).OrderByDescending(n => n.count);
            //changing slots and rooms
            if (orderedSlots != null)
            {
                foreach (var slotCount in orderedSlots)
                {
                    var slot = slotCount.slot;
                    var room = 0;
                    foreach (var oldSlot in timeTables.Where(n => n.Slot == slot))
                    {
                        oldSlot.RoomId = ++room;
                        dbContext.Entry(oldSlot).State = EntityState.Modified;
                    }
                }

                dbContext.SaveChanges();
            }

            return true;
        }

        public bool Delete(int id)
        {
            var t = dbContext.TimeTables.Where(n => n.ID == id).FirstOrDefault();
            if (t != null)
            {
                dbContext.TimeTables.Remove(t);
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }
        #endregion

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
