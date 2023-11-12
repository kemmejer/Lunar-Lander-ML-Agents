
public interface IOnEndEpisode
{
    public event OnEndEpisodeDelegate OnEndEpisodeEvent;

    public delegate void OnEndEpisodeDelegate(ShipAgent shipAgent);
}
