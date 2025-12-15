using System;
using System.Collections.Generic;

namespace InterDigital
{
    #region GachaMachine
    [Serializable]
    public class GachaMachineInventoryData
    {
        public string id;
        public int amount;

        public GachaMachineInventoryData(string inId, int inAmount)
        {
            id = inId;
            amount = inAmount;
        }
    }

    [Serializable]
    public class GachaMachinePrizeRecordData
    {
        public string id;
        public bool status;
        public long time;

        public GachaMachinePrizeRecordData(string inId, bool inStatus, long inTime)
        {
            id = inId;
            status = inStatus;
            time = inTime;
        }
    }
    #endregion

    #region ClawMachine
    [Serializable]
    public class ClawMachineInventoryData
    {
        public string id;
        public int amount;

        public ClawMachineInventoryData(string inId, int inAmount)
        {
            id = inId;
            amount = inAmount;
        }
    }

    [Serializable]
    public class ClawMachinePrizeRecordData
    {
        public string id;
        public bool status;
        public long time;

        public ClawMachinePrizeRecordData(string inId, bool inStatus, long inTime)
        {
            id = inId;
            status = inStatus;
            time = inTime;
        }
    }
    #endregion

    public class InventoryManager
    {
        GameManager gameManager;
        UserData userData;

        const int PRIZE_RECORD_LIMIT = 100;

        public void Init(GameManager inGameManager)
        {
            gameManager = inGameManager;
            userData = gameManager.userData;

            if (userData.gachaMachineData == null)
            {
                userData.gachaMachineData = new GachaMachineData();
                userData.gachaMachineData.Init();
            }

            if (userData.clawMachineData == null)
            {
                userData.clawMachineData = new ClawMachineData();
                userData.clawMachineData.Init();
            }
        }

        #region GachaMachine
        public void AddToGachaInventory(string id, int amount)
        {
            bool exist = IsGachaInventoryExist(id);

            if (exist)
            {
                GachaMachineInventoryData data = GetGachaMachineInventory(id);
                data.amount += amount;
            }
            else
            {
                GachaMachineInventoryData data = new GachaMachineInventoryData(id, amount);
                userData.gachaMachineData.inventoryDatas.Add(data);
            }
        }

        public bool IsGachaInventoryExist(string id)
        {
            List<GachaMachineInventoryData> list = userData.gachaMachineData.inventoryDatas;

            bool result = false;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                GachaMachineInventoryData data = list[i];
                if (data.id == id)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public GachaMachineInventoryData GetGachaMachineInventory(string inId)
        {
            List<GachaMachineInventoryData> list = userData.gachaMachineData.inventoryDatas;

            GachaMachineInventoryData result = null;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                GachaMachineInventoryData data = list[i];
                if (data.id == inId)
                {
                    result = data;
                    break;
                }
            }

            return result;
        }
        #endregion

        #region ClawMachine
        public void AddToClawInventory(string inId, int inAmount)
        {
            bool exist = IsClawInventoryExist(inId);

            if (exist)
            {
                ClawMachineInventoryData data = GetClawInvententory(inId);
                data.amount += inAmount;
            }
            else
            {
                ClawMachineInventoryData data = new ClawMachineInventoryData(inId, inAmount);
                userData.clawMachineData.inventoryDatas.Add(data);
            }
        }

        public void AddClawPrizeRecord(ClawMachinePrizeRecordData data)
        {
            List<ClawMachinePrizeRecordData> list = userData.clawMachineData.prizeRecordDatas;
            int count = list.Count;

            if (count == PRIZE_RECORD_LIMIT)
            {
                userData.clawMachineData.prizeRecordDatas.RemoveAt(0);
            }

            userData.clawMachineData.prizeRecordDatas.Add(data);
        }

        public bool IsClawInventoryExist(string inId)
        {
            List<ClawMachineInventoryData> list = userData.clawMachineData.inventoryDatas;

            bool result = false;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                ClawMachineInventoryData data = list[i];
                if (data.id == inId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public ClawMachineInventoryData GetClawInvententory(string inId)
        {
            List<ClawMachineInventoryData> list = userData.clawMachineData.inventoryDatas;

            ClawMachineInventoryData result = null;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                ClawMachineInventoryData data = list[i];
                if (data.id == inId)
                {
                    result = data;
                    break;
                }
            }

            return result;
        }
        #endregion
    }
}

