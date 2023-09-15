namespace RPA.ConsultaProcessosPrimeiroGrau.eSAJ.Handlers
{
    public interface ISeleniumHandler
    {
        void CloseChromeDriver();
        bool InitChromeDriver();
        bool Navigation();
    }
}
