import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.dates as mdates

file_path = 'Latency.csv'
df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])
# Convert to datetime
df['timestamp'] = pd.to_datetime(df['timestamp'])
# Set timestamp as index
df.set_index('timestamp', inplace=True)

print("Total number of data points in all_data:", len(df))

# Resample the data by second and calculate latency mean
df_resampled = df.resample('S').mean()  # S = sec, L = ms
# Interpolate missing values
df_resampled.interpolate(method='linear', inplace=True)
# Reset index
df_resampled.reset_index(inplace=True)

plt.figure(figsize=(10, 5))
plt.plot(df_resampled['timestamp'], df_resampled['latency'], label='Latency')
plt.title('Latency Evolution Over Time')
plt.xlabel('Time')
plt.ylabel('Latency (ms)')
plt.grid(True)

# Set the format for the x-axis to show time only
time_format = mdates.DateFormatter('%H:%M:%S')
plt.gca().xaxis.set_major_formatter(time_format)

plt.xticks(rotation=45)

# Add a horizontal line at 150 ms
plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

# Add a vertical line at each event
specific_time = pd.Timestamp('2024-05-10T10:32:30.0000000')
plt.axvline(x=specific_time, color='salmon', linestyle='--', label='Code deployed')

specific_time = pd.Timestamp('2024-05-10T10:36:10.0000000')
plt.axvline(x=specific_time, color='salmon', linestyle='--', label='Image push to Google Cloud')

specific_time = pd.Timestamp('2024-05-10T10:36:15.0000000')
plt.axvline(x=specific_time, color='salmon', linestyle='--', label='Creating new instances')

specific_time = pd.Timestamp('2024-05-10T10:36:40.0000000')
plt.axvline(x=specific_time, color='salmon', linestyle='--', label='New instances running')

plt.legend()
plt.tight_layout()
plt.show()
