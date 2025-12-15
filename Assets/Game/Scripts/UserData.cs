using System;
using System.Collections.Generic;

namespace InterDigital
{
    public class UserData
    {
        public long lastPlayTime;
        public int coin;
        public int diamond;

        public GachaMachineData gachaMachineData;
        public ClawMachineData clawMachineData;

        public void Init(bool firstTimePlay = false)
        {
            if (firstTimePlay)
            {
                lastPlayTime = 0;
                coin = 0;
                diamond = 0;
            }
        }
    }

    [Serializable]
    public class GachaMachineData
    {
        public List<GachaMachineInventoryData> inventoryDatas;
        public List<GachaMachinePrizeRecordData> prizeRecordDatas;

        public void Init()
        {
            if (inventoryDatas == null)
            {
                inventoryDatas = new List<GachaMachineInventoryData>();
            }

            if (prizeRecordDatas == null)
            {
                prizeRecordDatas = new List<GachaMachinePrizeRecordData>();
            }
        }
    }

    [Serializable]
    public class ClawMachineData
    {
        public int currentPity;

        public List<ClawMachineInventoryData> inventoryDatas;
        public List<ClawMachinePrizeRecordData> prizeRecordDatas;

        public List<List<bool>> probabilityDatas;

        public void Init()
        {
            currentPity = 0;

            if (inventoryDatas == null)
            {
                inventoryDatas = new List<ClawMachineInventoryData>();
            }

            if (prizeRecordDatas == null)
            {
                prizeRecordDatas = new List<ClawMachinePrizeRecordData>();
            }
        }
    }
}

