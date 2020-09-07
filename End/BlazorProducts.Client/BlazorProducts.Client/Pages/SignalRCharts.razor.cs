using BlazorProducts.Client.HttpRepository;
using Entities.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorProducts.Client.Pages
{
	public partial class SignalRCharts : IDisposable
	{
		private HubConnection _hubConnection;
		private List<ChartDto> _data;

		public List<ChartDto> Data = new List<ChartDto>();
		[Inject]
		public IProductHttpRepository Repo { get; set; }

		protected async override Task OnInitializedAsync()
		{
			await StartHubConnection();

			AddTransferChartDataListener();

			await Repo.CallChartEndpoint();
		}

		private async Task StartHubConnection()
		{
			_hubConnection = new HubConnectionBuilder()
							.WithUrl("https://localhost:5011/chart")
							.Build();

			await _hubConnection.StartAsync();
			if (_hubConnection.State == HubConnectionState.Connected)
				Console.WriteLine("connection started");
		}

		private void AddTransferChartDataListener()
		{
			_hubConnection.On<List<ChartDto>>("transferchartdata", (_data) =>
			{
				foreach (var item in _data)
				{
					Console.WriteLine($"Label: {item.Label}, Value: {item.Value}");
				}

				Data = _data;
				StateHasChanged();
			});
		}

		public void Dispose()
		{
			_hubConnection.DisposeAsync();
		}
	}
}
