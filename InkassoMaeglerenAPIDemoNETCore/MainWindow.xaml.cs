using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SystemOneService;

namespace InkassoMaeglerenAPIDemoNETCore
{
    public partial class MainWindow : Window
    {
        SystemOneSoapServiceClient soapClient;
        HttpClient httpClient;            

        enum API { SOAP, REST }

        API currentAPI = API.REST;

        string serviceAddress = "https://inkassomaeglerenwcfapidevelop.azurewebsites.net/SystemOneSoapService.svc";

        string apiToken = "";
        int DEMOCASETYPEID = ;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            soapClient = new SystemOneSoapServiceClient();
            soapClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceAddress + "/soap");
            //client.Endpoint.Binding.SendTimeout = new TimeSpan(0, 0, 10);
            soapClient.Endpoint.Binding.SendTimeout = new TimeSpan(0, 10, 0); ///While Debugging


            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(serviceAddress + "/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void btn_TestConnectionGet_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = string.Empty;

            try
            {
                switch (currentAPI)
                {
                    case API.SOAP:
                        await soapClient.TestConnectionGetAsync();

                        break;
                    case API.REST:
                        HttpResponseMessage response = await httpClient.GetAsync("json/TestConnectionGet");
                        string content = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            errorMessage = "Success";
                        }
                        else
                        {
                            errorMessage = content;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            tb_Output.Text = errorMessage;
        }

        private async void btn_TestConnectionPost_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                switch (currentAPI)
                {
                    case API.SOAP:
                        await soapClient.TestConnectionPostAsync();
                        break;
                    case API.REST:
                        string postBody = String.Empty;

                        HttpResponseMessage response = await httpClient.PostAsJsonAsync("json/TestConnectionPost", postBody);
                        string content = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            errorMessage = "Success";
                        }
                        else
                        {
                            errorMessage = content;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            tb_Output.Text = errorMessage;
        }

        private async void btn_GetActs_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var output = await soapClient.GetActsAsync(apiToken, 0, DEMOCASETYPEID, null, null, null);

            }
            catch (Exception ex)
            {

            }


        }

        private void btn_ExitAct_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btn_Deposit_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = string.Empty;

            string refNo = "7444";
            //string refNo = "666"; ///Don't use the same refno as the one at import new case, because it will not be correct if u import it on case that has just been imported, due to some special deposit functionality. Also this function will fail if u don't import the case first. 

            IncomingDeposit deposit = new IncomingDeposit
            {
                Amount = 100,
                DepositDate = DateTime.Today.ToString("yyyy-MM-dd"),
            };

            try
            {
                switch (currentAPI)
                {
                    case API.SOAP:
                        await soapClient.DepositAsync(apiToken, 0, refNo, deposit);
                        break;
                    case API.REST:
                        string postBody = JsonConvert.SerializeObject(deposit);

                        var actImportEfter = JsonConvert.DeserializeObject<IncomingDeposit>(postBody);

                        HttpResponseMessage response = await httpClient.PostAsJsonAsync("json/Deposit?token=" + apiToken + "&versionNumber=0" + "&caseReferenceId=" + refNo, deposit);
                        string content = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            errorMessage = "Success";
                        }
                        else
                        {
                            errorMessage = content;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            tb_Output.Text = errorMessage;
        }

        private async void btn_ImportActDirect_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = string.Empty;

            string refNo = "666";

            Dictionary<string, string> actExtraFields = new Dictionary<string, string>();
            //actExtraFields.Add("Telefon1", "456546547");
            //actExtraFields.Add("Telefon2", "456546547");

            ActExtraFields extraFields = new ActExtraFields
            {
                ExtraFields = actExtraFields,
            };


            List<Debtor> debtors = new List<Debtor>();
            debtors.Add(
                new Debtor
                {
                    address = "Kratbjerg 201",
                    //birthday = new DateTime(2012, 12, 12),
                    countryCode = "DK",
                    cpr = null,
                    cvr = "11223344",
                    email = "testmail@testmail.nu",
                    name = "Testfirma",
                    postalCode = "3480",
                    city = "Fredensborg",
                });


            List<Debt> debts = new List<Debt>();
            debts.Add(
                new Debt
                {
                    interestRate = null,
                    //interestStartDate = null,
                    invoiceDate = new DateTime(2021, 12, 12).ToString("yyyy-MM-dd"),
                    dueDate = new DateTime(2021, 12, 24).ToString("yyyy-MM-dd"),
                    invoiceId = "1",
                    principal = 5000,
                    fee = 100,
                    compensationFee = null,
                    otherFees = null,
                });

            Act act = new Act
            {
                caseTypeId = DEMOCASETYPEID,
                caseReferenceId = refNo,
                extraFields = extraFields,
                debtors = debtors,
                debts = debts,
            };

            bool allDebtsBelongToAllDebtors = true;

            try
            {
                switch (currentAPI)
                {
                    case API.SOAP:
                        await soapClient.ImportActDirectAsync(apiToken, 0, allDebtsBelongToAllDebtors, act);
                        break;
                    case API.REST:
                        string postBody = JsonConvert.SerializeObject(act);

                        var actImportEfter = JsonConvert.DeserializeObject<Act>(postBody);

                        HttpResponseMessage response = await httpClient.PostAsJsonAsync("json/ImportActDirect?token=" + apiToken + "&versionNumber=0" + "&allDebtsBelongToAllDebtors=" + allDebtsBelongToAllDebtors, act);
                        string content = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            errorMessage = "Success";
                        }
                        else
                        {
                            errorMessage = content;
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            tb_Output.Text = errorMessage;
        }

        private void btn_UpdateAct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ImportActToFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_CreatePaymentPlan_Click(object sender, RoutedEventArgs e)
        {
            //await client.CreatePaymentPlanAsync(UIControlsWPF.Tools.systemUser.ApiToken, 0, act.Id, debtor.Id, startDate, payAmount, debtIds);
        }

        private void btn_APISwitch_Click(object sender, RoutedEventArgs e)
        {
            switch (currentAPI)
            {
                case API.SOAP:
                    currentAPI = API.REST;
                    break;
                case API.REST:
                    currentAPI = API.SOAP;
                    break;
                default:
                    throw new NotSupportedException();
            }

            btn_APISwitch.Content = currentAPI.ToString() + " " + "Active";
        }
    }
}
