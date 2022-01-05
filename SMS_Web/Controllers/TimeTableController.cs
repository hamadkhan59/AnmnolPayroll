using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.DTOs;
using SMS_Web.Helpers;
using SMS_Web.MakeClassSchedule.Algorithm;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers
{
    public class TimeTableController : Controller
    {
        ITimeTableRepository timeTableRepo;
        public static Algorithm AA;
        public static ThreadState state = ThreadState.Unstarted;
        public TimeTableController()
        {
            timeTableRepo = new TimeTableRepositoryImp(new SC_WEBEntities2());
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_TIME_TABLE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            var sched = timeTableRepo.GetViewModelByBranch(UserPermissionController.GetLoginBranchId(Session.SessionID));
            if (sched != null)
            {
                ViewData["ScheduleStatus"] = "Exist";
            }
            else
            {
                ViewData["ScheduleStatus"] = "New";
            }

            return View("TimeTable", sched);
        }

        public JsonResult GetSchedule()
        {
            return Json(timeTableRepo.GetViewModelByBranch(UserPermissionController.GetLoginBranchId(Session.SessionID)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult StartAlgorithm()
        {
            Configuration.GetInstance.ParseFile(Session.SessionID);
            if (Configuration.GetInstance.GetNumberOfRooms() > 0)
            {
                Schedule prototype = new Schedule(5, 5, 90, 10);
                AA = new MakeClassSchedule.Algorithm.Algorithm(1000, 180, 50, prototype, null);
                state = ThreadState.Unstarted;
                ViewData["AlgorithmStatus"] = "Running";
                AA.Start();
                state = ThreadState.Running;
                return Json(new JSONResponse() { Success = true, Message = "Running Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new JSONResponse() { Success = false, Message = "You havn't set up data correctly. Please assign subjects to teachers first." }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult StopAlgorithm(bool save = false)
        {
            AA.Stop();
            state = ThreadState.Suspended;
            if (save)
            {
                Save();
                return Json(new JSONResponse() { Success = true, Message = "Algorithm Stopped Successfully." }, JsonRequestBehavior.AllowGet);
            }
            ViewData["AlgorithmStatus"] = "Stopped";
            return Json(new JSONResponse() { Success = true, Message = "Algorithm Stopped Successfully." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PauseAlgorithm()
        {
            AA.Pause();
            state = ThreadState.Stopped;
            ViewData["AlgorithmStatus"] = "Paused";
            return Json(new JSONResponse() { Success = true, Message = "Algorithm Paused Successfully." }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ResumeAlgorithm()
        {
            AA.Resume();
            state = ThreadState.Running;
            ViewData["AlgorithmStatus"] = "Running";
            return Json(new JSONResponse() { Success = true, Message = "Algorithm Resumed Successfully." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveScheduleChanges(List<LectureSlot> LectureSlots)
        {
            timeTableRepo.SaveChanges(UserPermissionController.GetLoginBranchId(Session.SessionID), LectureSlots);
            return Json(new JSONResponse() { Success = true, Message = "Changes Saved Successfully." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFitness()
        {
            if (state == ThreadState.Unstarted)
            {
                dynamic obj = new ExpandoObject();
                obj.State = "Unstarted";
                obj.Fitness = 0;
                return Json(new JSONResponse() { Source = obj, Success = true, Message = "Currently no action performed on schedule." }, JsonRequestBehavior.AllowGet);
            }

            var fitness = Algorithm.GetInstance().GetBestChromosome().GetFitness();
            if (fitness >= 1)
            {
                Save();
                dynamic obj = new ExpandoObject();
                obj.State = "Completed";
                obj.Fitness = 1;
                return Json(new JSONResponse() { Source = obj, Success = true, Message = "Schedule Completed Successfully." }, JsonRequestBehavior.AllowGet);
            }
            else if (state == ThreadState.Running)
            {
                dynamic obj = new ExpandoObject();
                obj.State = "Running";
                obj.Fitness = fitness;
                return Json(new JSONResponse() { Source = obj, Success = true, Message = "Schedule is processing." }, JsonRequestBehavior.AllowGet);
            }
            else if (state == ThreadState.Stopped)
            {
                dynamic obj = new ExpandoObject();
                obj.State = "Paused";
                obj.Fitness = fitness;
                return Json(new JSONResponse() { Source = obj, Success = true, Message = "Schedule is Paused." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                dynamic obj = new ExpandoObject();
                obj.State = "Unstarted";
                obj.Fitness = 0;
                return Json(new JSONResponse() { Source = obj, Success = true, Message = "Currently no action performed on schedule." }, JsonRequestBehavior.AllowGet);
            }
        }
        private string Save()
        {
            Schedule schedule = Algorithm.GetInstance().GetBestChromosome();

            int numberOfRooms = Configuration.GetInstance.GetNumberOfRooms();
            int daySize = schedule.day_Hours * numberOfRooms;
            var timeTables = new List<TimeTable>();
            var branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

            foreach (KeyValuePair<CourseClass, int> it in schedule.GetClasses().ToList())
            {
                // coordinate of time-space slot
                int pos = it.Value; // int pos of _slot array
                int day = pos / daySize;
                int time = pos % daySize; // this is not time now!
                int room = time / schedule.day_Hours;
                time = time % schedule.day_Hours;  // this is a time now!
                int dur = it.Key.GetDuration;

                CourseClass cc = it.Key;
                Room r = Configuration.GetInstance.GetRoomById(room);
                //
                // Save Classroom_Time
                //
                var timeTable = new TimeTable();
                timeTable.BranchId = branchId;
                timeTable.Day = day;
                timeTable.Slot = time;
                timeTable.RoomId = cc.Room_ID;
                timeTable.TeacherId = cc.GetProfessor.GetId;
                timeTable.ClassSubjectId = cc.GetCourse.GetId;

                //db.Classroom_TimeSave(r.Origin_ID_inDB, cc.Class_ID, cc.GetProfessor.GetId, time, dur, day);
                //
                // Save New_GroupsPerClassroom
                //
                foreach (var gs in cc.GetGroups)
                {
                    timeTable.ClassSectionId = gs.GetId;
                    timeTables.Add(timeTable);
                    //db.New_GroupsPerClassSave(r.Origin_ID_inDB, cc.Class_ID, time, day, gs.GetId);
                }

            }

            var orderedSlots = timeTables.GroupBy(n => n.Slot).Select(n => new { slot = n.Key, count = n.Count() }).OrderByDescending(n => n.count);
            //changing slots and rooms
            var updatedTables = new List<TimeTable>();
            if (orderedSlots != null)
            {
                var period = 1;
                foreach (var slotCount in orderedSlots)
                {
                    var slot = slotCount.slot;
                    var room = 0;
                    foreach (var oldSlot in timeTables.Where(n => n.Slot == slot))
                    {
                        var timeTable = new TimeTable();
                        timeTable.BranchId = oldSlot.BranchId;
                        timeTable.Day = oldSlot.Day;
                        timeTable.Slot = period;
                        timeTable.RoomId = ++room;
                        timeTable.TeacherId = oldSlot.TeacherId;
                        timeTable.ClassSubjectId = oldSlot.ClassSubjectId;
                        timeTable.ClassSectionId = oldSlot.ClassSectionId;

                        updatedTables.Add(timeTable);
                    }

                    period++;
                }
            }

            timeTableRepo.CreateOrUpdate(branchId, updatedTables);
            return null;// "Changes not saved because algorithm has not found good solution yet.";
        }


        //int StartedTick = 0;E:\yousaf work\Git Projects\SMS_WEB
        //object TimerControler = new object();
        //private void timerWorkingSet_Tick()
        //{
        //    long memByte = Environment.WorkingSet;
        //    long memKByte = memByte / 1024;
        //    int memMByte = (int)(memKByte / 1024);

        //    if (state == ThreadState.Running || state == ThreadState.WaitSleepJoin || state == ThreadState.Suspended)
        //    {
        //        int timeLenght = (Environment.TickCount - StartedTick) / 1000; // Convert to Second

        //        string S = (timeLenght % 60).ToString();
        //        string M = ((timeLenght / 60) % 60).ToString();
        //        string H = (timeLenght / 3600).ToString();
        //        S = (S.Length > 1) ? S : S.Insert(0, "0");
        //        M = (M.Length > 1) ? M : M.Insert(0, "0");
        //        H = (H.Length > 1) ? H : H.Insert(0, "0");
        //        var msg = string.Format("Result             Working Time ({0}:{1}:{2})", H, M, S);
        //    }

        //    //Monitor.Enter(TimerControler);
        //    //if (PCF != null)
        //    //    if (!PCF.IsDisposed)
        //    //        PCF.PC_ActivityMonitor.RefreshData();
        //    //Monitor.Exit(TimerControler);

        //    //
        //    // check end of solving
        //    //
        //    if (Algorithm._state == AlgorithmState.AS_CRITERIA_STOPPED)
        //    {
        //        Save();
        //        Algorithm._state = AlgorithmState.AS_USER_STOPPED;
        //    }
        //}
    }
}
