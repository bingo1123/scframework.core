namespace YS.Yuanji.Drive
{
    public interface IDevice
    {
        Task<bool> StartAsync();

        Task<bool> StoptAsync();

        Task<bool> InitAsync(IChanlel chanlel, DeviceConfig device, List<ParameterItems> dataConfig);

    }
}
