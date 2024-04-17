using System.Collections.Concurrent;

namespace Solution;

public class UserStateManager
{
    private readonly ConcurrentDictionary<long, UserState> _userStates = new();

    public UserState this[long userId] => _userStates.GetOrAdd(userId, new UserState());
}