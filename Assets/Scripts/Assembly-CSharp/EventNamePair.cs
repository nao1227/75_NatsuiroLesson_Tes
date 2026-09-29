using System;
using Paidia.satsuki1;
using Serialize;

[Serializable]
public class EventNamePair : KeyAndValue<EventName, OsawariEvent>
{
	public EventNamePair(EventName key, OsawariEvent value)
		: base(key, value)
	{
	}
}
