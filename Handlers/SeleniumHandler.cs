using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RPA.ConsultaProcessosPrimeiroGrau.eSAJ.Handlers
{
    public class SeleniumHandler : ISeleniumHandler
    {
        private readonly ILogger<ISeleniumHandler> _logger;
        private IWebDriver _driver;
        private readonly string _url;

        public SeleniumHandler(ILogger<ISeleniumHandler> logger, IConfiguration configuration) 
        {
            _logger = logger;
            _url = configuration.GetValue<string>("UrlPage") ?? string.Empty;
        }

        #region PublicMethods
        public bool InitChromeDriver()
        {
            try
            {
                ChromeOptions options = new()
                {
                    PageLoadStrategy = PageLoadStrategy.Normal
                };

                options.AddArgument("no-sandbox");
                options.AddArgument("--profile-directory=Default");
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--start-maximized");
                options.AddExcludedArgument("enable-automation");

                //Desativar logs da pagina.
                options.AddExcludedArgument("enable-logging");

                //Alterar Proxy
                options.Proxy = new Proxy { Kind = ProxyKind.System };

                _driver = new ChromeDriver(options);

                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                _driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar ChromeDriver");
                return false;
            }
        }

        public void CloseChromeDriver()
        {
            try
            {
                _driver.Quit();
            }
            catch { }
        }

        public bool Navigation()
        {
            bool result = false;
            try
            {
                _driver.Navigate().GoToUrl(_url);

                IWebElement? selectSearchBy = _driver.FindElement(By.Id("cbPesquisa"));
                if (selectSearchBy is not null)
                {
                    selectSearchBy.Click();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    selectSearchBy.FindElement(By.XPath("option[@value='NMPARTE']")).Click();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    selectSearchBy.Click();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));

                    IWebElement? inputConsult = _driver.FindElement(By.Id($"campo_NMPARTE"));
                    if (inputConsult is not null)
                    {
                        inputConsult.SendKeys("MARCELO SANTOS");
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));

                        _driver.FindElement(By.Id("pesquisarPorNomeCompleto")).Click();
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));

                        SelectOption("4ª Vara Juizado Especial de Campo Grande");
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        SelectOption("Todas comarcas");
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        IWebElement? inputConsultProcess = _driver.FindElement(By.Id("botaoConsultarProcessos"));
                        if (inputConsultProcess is not null)
                        {
                            inputConsultProcess.Click();
                            Thread.Sleep(TimeSpan.FromSeconds(1));

                            IWebElement? linkProcess = _driver.FindElement(By.XPath("//div[@id='listagemDeProcessos']/ul//a[@class='linkProcesso'][1]"));
                            if (linkProcess is not null)
                            {
                                linkProcess.Click();
                                Thread.Sleep(TimeSpan.FromSeconds(1));

                                IWebElement? classProcess = _driver.FindElement(By.Id("classeProcesso"));

                                if (classProcess is not null)
                                    _logger.LogInformation($"Classe: {classProcess.Text.Trim()}");
                                else
                                    _logger.LogInformation($"Classe: Não encontrada");

                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de Navegação");
            }

            return result;
        }
        #endregion

        #region PrivateMethods
        private void SelectOption(string value)
        {
            IWebElement? select = _driver.FindElement(By.XPath("//div[@id='s2id_comboForo']/a"));
            if (select is not null)
            {
                select.Click();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));

                IWebElement? option = _driver.FindElement(By.XPath($"//ul[@id='select2-results-1']/li/div[contains(., '{value}')]"));
                if (option is not null)
                {
                    option.Click();
                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            }
        }
        #endregion
    }
}
