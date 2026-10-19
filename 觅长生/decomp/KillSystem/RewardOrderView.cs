using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KillSystem;

public class RewardOrderView : MonoBehaviour
{
	public PlayerSetRandomFace Face;

	public Text NpcName;

	public Text ChengHao;

	public Text Level;

	public List<Image> RewardOrderLevel;

	public Text RewardMoney;

	public FpBtn AcceptBtn;

	public RewardOrderModel Model;
}
