using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAI
{
    class TempFileManager
    {
        public static void GetStats(string folder)
        {
            // Example parameter: "depots/drive/100"
            string BbcRelativePath = "../../../../../BBC";
            string path = BbcRelativePath + "/domainfiles/" + folder;

            FileHelperEngine<Temp0> tempEngine0 = new();
            FileHelperEngine<Temp1> tempEngine1 = new();
            FileHelperEngine<Temp2> tempEngine2 = new();
            Temp0[] t0 = tempEngine0.ReadFile(path + "/temp0.csv");
            Temp1[] t1 = tempEngine1.ReadFile(path + "/temp1.csv");
            Temp2[] t2 = tempEngine2.ReadFile(path + "/temp2.csv");

            Stats stats = new Stats()
            {
                Domain = t0[0].Domain,
                Action = t0[0].Action,
                Beta = t1[0].Beta,
                ValidationPercent = 0,
                TestdataPercent = t0[0].TestdataPercent,
                TrainingdataPercent = t0[0].TrainingdataPercent,
                NumProblems = t0[0].NumProblems,
                NumusefulActions = t0[0].NumusefulActions,
                NumuselessActions = t0[0].NumuselessActions,
                Vars = t2[0].Vars,
                Clauses = t2[0].Clauses,
                Body = t2[0].Body,
                TpTraining = t1[0].TpTraining,
                TnTraining = t1[0].TnTraining,
                FpTraining = t1[0].FpTraining,
                FnTraining = t1[0].FnTraining,
                RecallTraining = t1[0].RecallTraining,
                PrecisionTraining = t1[0].PrecisionTraining,
                TpTest = t2[0].TpTest,
                TnTest = t2[0].TnTest,
                FpTest = t2[0].FpTest,
                FnTest = t2[0].FnTest,
                RecallTest = t2[0].RecallTest,
                PrecisionTest = t2[0].PrecisionTest,
                TpValidation = 0,
                TnValidation = 0,
                FpValidation = 0,
                FnValidation = 0,
                RecallValidation = 0,
                PrecisionValidation = 0
            };

            FileHelperEngine<Stats> statsEngine = new();
            statsEngine.AppendToFile(BbcRelativePath + "/StatsFile.csv", stats);
        }
    }

    [DelimitedRecord(",")]
    class Temp0
    {
        public string Domain;
        public string Action;
        public float TestdataPercent;
        public float TrainingdataPercent;
        public int NumProblems;
        public int NumusefulActions;
        public int NumuselessActions;
    }

    [DelimitedRecord(",")]
    class Temp1
    {
        public int Beta;
        public int TpTraining;
        public int FnTraining;
        public int TnTraining;
        public int FpTraining;
        public float PrecisionTraining;
        public float RecallTraining;
    }

    [DelimitedRecord(",")]
    class Temp2
    {
        public int Vars;
        public int Clauses;
        public int Body;
        public int TpTest;
        public int TnTest;
        public int FpTest;
        public int FnTest;
        public float RecallTest;
        public float PrecisionTest;
    }

    [DelimitedRecord(";")]
    class Stats
    {
        public string Domain;
        public string Action;
        public int Beta;
        public float ValidationPercent;
        public float TestdataPercent;
        public float TrainingdataPercent;
        public int NumProblems;
        public int NumusefulActions;
        public int NumuselessActions;
        public int Vars;
        public int Clauses;
        public int Body;
        public int TpTraining;
        public int TnTraining;
        public int FpTraining;
        public int FnTraining;
        public float RecallTraining;
        public float PrecisionTraining;
        public int TpTest;
        public int TnTest;
        public int FpTest;
        public int FnTest;
        public float RecallTest;
        public float PrecisionTest;
        public int TpValidation;
        public int TnValidation;
        public int FpValidation;
        public int FnValidation;
        public float RecallValidation;
        public float PrecisionValidation;
    }
}
