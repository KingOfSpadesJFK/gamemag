public interface ITimedData<T>
{
    public void AppendAtStart(T data);
    public void AppendAtEnd(T data);
    public void Append(T data);
    public void Reset();
	public void Invert();
    public void SetEndPoint(bool inverted);
    public void SetStartPoint(bool inverted);
	public void StartPlaybackAtBeginning();
	public void StartPlaybackAtEnding();

    public T Get(int time);
	public T Get(int minute, int second, int tick);
    public T Next();
    public T Previous();
    public T Seek(int time);

}