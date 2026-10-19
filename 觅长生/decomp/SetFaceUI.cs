using System;
using System.Runtime.CompilerServices;
using KBEngine;
using UnityEngine;
using UnityEngine.Events;
using script.SetFace;

public class SetFaceUI : MonoBehaviour, IESCClose
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__6_1;

		internal void _003COpen_003Eb__6_1()
		{
			Open(1);
		}
	}

	public static SetFaceUI Inst;

	public MainUISetFaceB SetFaceB;

	public PlayerSetRandomFace Face;

	public JSONObject OldFaceData;

	public GameObject Lock;

	public int NpcId = 1;

	public static void Open()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		Avatar player = Tools.instance.getPlayer();
		UnityAction val = delegate
		{
			if (player.hasItem(5322))
			{
				Tools.instance.RemoveItem(5322);
				player.IsCanSetFace = true;
				Open(1);
			}
			else
			{
				UIPopTip.Inst.Pop("易容丹数量不足");
			}
		};
		object obj = _003C_003Ec._003C_003E9__6_1;
		if (obj == null)
		{
			UnityAction val2 = delegate
			{
				Open(1);
			};
			_003C_003Ec._003C_003E9__6_1 = val2;
			obj = (object)val2;
		}
		USelectBox.Show("是否要服用易容丹", val, (UnityAction)obj);
	}

	public static void Open(int npcId)
	{
		npcId = NPCEx.NPCIDToNew(npcId);
		if (jsonData.instance.AvatarRandomJsonData[npcId.ToString()]["a_suit"].I < 90 || jsonData.instance.AvatarRandomJsonData[npcId.ToString()]["a_suit"].I > 100)
		{
			ResManager.inst.LoadPrefab("SetFaceUI").Inst(((Component)NewUICanvas.Inst).transform);
			Inst.NpcId = npcId;
			Inst.OldFaceData = jsonData.instance.AvatarRandomJsonData["1"].Copy();
			jsonData.instance.AvatarRandomJsonData.SetField("1", jsonData.instance.AvatarRandomJsonData[npcId.ToString()].Copy());
			Inst.Init();
		}
		else
		{
			UIPopTip.Inst.Pop("此NPC不支持换装");
		}
	}

	private void Awake()
	{
		Inst = this;
	}

	public void Init()
	{
		Face.setFace();
		Lock.SetActive(!Tools.instance.getPlayer().IsCanSetFace);
		if (Tools.instance.getPlayer().IsCanSetFace)
		{
			Tools.instance.getPlayer().IsCanSetFace = false;
		}
		ESCCloseManager.Inst.RegisterClose(this);
		((Component)this).transform.SetAsLastSibling();
		AvatarFaceDatabase.inst.ListType = GetSex();
		SetFaceB.Init();
	}

	public int GetSex()
	{
		return jsonData.instance.AvatarRandomJsonData["1"]["Sex"].I;
	}

	private void OnDestroy()
	{
		ESCCloseManager.Inst.UnRegisterClose(this);
		Inst = null;
	}

	public void Cancel()
	{
		jsonData.instance.AvatarRandomJsonData.SetField("1", OldFaceData);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void Ok()
	{
		Close();
		if (NpcId > 1)
		{
			Object.Instantiate<GameObject>(Resources.Load<GameObject>("talkPrefab/TalkPrefab/talk4026"));
		}
	}

	public void Close()
	{
		JSONObject obj = jsonData.instance.AvatarRandomJsonData["1"].Copy();
		jsonData.instance.AvatarRandomJsonData.SetField("1", OldFaceData);
		jsonData.instance.AvatarRandomJsonData.SetField(NpcId.ToString(), obj);
		UIHeadPanel.Inst.Face.SetNPCFace(1);
		if ((Object)(object)UINPCLeftList.Inst != (Object)null)
		{
			UINPCLeftList.Inst.needRefresh = true;
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public bool TryEscClose()
	{
		Cancel();
		return true;
	}
}
