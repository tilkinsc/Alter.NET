using Exceptions;

namespace Game.Model;

class Hitmark
{
	public int Type;
	public int Damage;
	
	public Hitmark(int type, int damage)
	{
		Type = type;
		Damage = damage;
	}
}

class Hitbar
{
	public int Type;
	public int Percentage;
	public int MaxPercentage;
	public int DepleteSpeed;
	public int Delay;
	
	public Hitbar(int type, int percentage, int maxPercentage, int depleteSpeed, int delay)
	{
		Type = type;
		Percentage = percentage;
		MaxPercentage = maxPercentage;
		DepleteSpeed = depleteSpeed;
		Delay = delay;
	}
}

class Hit
{
	
	public List<Hitmark> Hitmarks;
	public Hitbar? Hitbar;
	public List<Action> Actions = new List<Action>();
	public Func<bool> CancelCondition = () => false;
	public int ClientDelay;
	public int DamageDelay;
	
	private Hit(List<Hitmark> hitmarks, Hitbar? hitbar, int clientDelay, int damageDelay)
	{
		Hitmarks = hitmarks;
		Hitbar = hitbar;
	}
	
	public Hit AddAction(Action action)
	{
		Actions.Add(action);
		return this;
	}
	
	public Hit AddActions(List<Action> actions)
	{
		Actions.AddRange(actions);
		return this;
	}
	
	public Hit SetCancelIf(Func<bool> condition)
	{
		CancelCondition = condition;
		return this;
	}
	
	public class Builder
	{
		
		private List<Hitmark> Hitmarks = new List<Hitmark>();
		private bool OnlyShowHitbar;
		private bool HideHitbar;
		private int HitbarType;
		private int HitbarPercentage;
		private int HitbarMaxPercentage;
		private int HitbarDepleteSpeed;
		private int HitbarDelay;
		private int ClientDelay;
		private int DamageDelay;
		
		public Hit Build()
		{
			if (OnlyShowHitbar == false && Hitmarks.Count == 0)
				throw new IllegalStateException("You can't build a Hit with not hitmarkers unless OnlyShowHitbar is set");
			if (OnlyShowHitbar && HideHitbar)
				throw new IllegalStateException("You can't have both OnlyShowHitbar and HideHitbar set");
			if (!HideHitbar && HitbarDepleteSpeed > 0 && HitbarMaxPercentage > 0)
				throw new IllegalStateException("You can't set HitbarDepleteSpeed > 0 unless HitbarMaxPercentage > 0");
			
			Hitbar? hitbar = !HideHitbar ? new Hitbar(HitbarType, HitbarPercentage, HitbarMaxPercentage, HitbarDepleteSpeed, HitbarDelay) : null;
			return new Hit(Hitmarks, hitbar, ClientDelay, DamageDelay);
		}
		
		public Builder AddHit(int damage, int type)
		{
			Hitmarks.Add(new Hitmark(type, damage));
			return this;
		}
		
		public Builder SetClientDelay(int delay)
		{
			ClientDelay = delay;
			return this;
		}
		
		public Builder SetDamageDelay(int delay)
		{
			DamageDelay = delay;
			return this;
		}
		
		public Builder SetOnlyShowHitbar(bool onlyShowHitbar)
		{
			OnlyShowHitbar = onlyShowHitbar;
			return this;
		}
		
		public Builder SetHideHitbar(bool hide)
		{
			HideHitbar = hide;
			return this;
		}
		
		public Builder SetHitbarType(int type)
		{
			HitbarType = type;
			return this;
		}
		
		public Builder SetHitbarDepleteSpeed(int depleteSpeed)
		{
			HitbarDepleteSpeed = depleteSpeed;
			return this;
		}
		
		public Builder SetHitbarPercentage(int percentage)
		{
			HitbarPercentage = percentage;
			return this;
		}
		
		public Builder SetHitbarMaxPercentage(int percentage)
		{
			HitbarMaxPercentage = percentage;
			return this;
		}
		
		public Builder SetHitbarDelay(int delay)
		{
			HitbarDelay = delay;
			return this;
		}
		
	}
	
}
