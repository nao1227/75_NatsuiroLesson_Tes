using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
	private Animator animator;

	private int KaihatsuLevel;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public int GetKaihatsuLevel()
	{
		return KaihatsuLevel;
	}

	public void LevelUpKaihatsuLevel()
	{
		KaihatsuLevel++;
		int integer = animator.GetInteger("Kaihatsu_Level");
		animator.SetInteger("Kaihatsu_Level", integer + 1);
	}
}
