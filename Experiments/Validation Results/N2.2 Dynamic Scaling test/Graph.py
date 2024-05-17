import os
import pandas as pd
import matplotlib.pyplot as plt
 
directory = 'Latency'

# Initialize an empty DataFrame to hold all the data
all_data = pd.DataFrame()

# Loop through each file in the directory
for filename in os.listdir(directory):
    if filename.endswith('.csv'):
        file_path = os.path.join(directory, filename)
        # Read each CSV file
        df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])
        # Convert 'timestamp' to datetime
        df['timestamp'] = pd.to_datetime(df['timestamp'])
        # Append the data from this file to the main DataFrame
        all_data = pd.concat([all_data, df])

# Set timestamp as index
all_data.set_index('timestamp', inplace=True)

print("Total number of data points in all_data:", len(all_data))

# Resample the data by second and calculate latency mean
df_resampled = all_data.resample('S').mean()  # S = sec, L = ms
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
plt.xticks(rotation=45)

# Add a horizontal line at 150 ms
plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

# Add a vertical line at each event
specific_time = pd.Timestamp('2024-05-10T13:46:16.6405422')
plt.axvline(x=specific_time, color='salmon', linestyle='--', label='Client increase of 10')
specific_time = pd.Timestamp('2024-05-10T13:47:16.7021654')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:48:16.7334483')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:49:16.7633638')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:50:16.7995856')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:51:16.8295458')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:52:16.8563998')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:53:16.8940659')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:54:16.9345776')
plt.axvline(x=specific_time, color='salmon', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:55:16.9762407')
plt.axvline(x=specific_time, color='salmon', linestyle='--')

                                                            # I W C
specific_time = pd.Timestamp('2024-05-10T13:46:55.0000000') # 4 4 4
plt.axvline(x=specific_time, color='springgreen', linestyle='--', label='Services increase')
specific_time = pd.Timestamp('2024-05-10T13:47:12.0000000') # 5 4 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:47:42.0000000') # 6 5 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:47:57.0000000') # 7 5 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:48:12.0000000') # 7 6 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:48:42.0000000') # 7 7 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:48:58.0000000') # 9 7 6
plt.axvline(x=specific_time, color='springgreen', linestyle='--')
specific_time = pd.Timestamp('2024-05-10T13:49:12.0000000') # 10 9 8
plt.axvline(x=specific_time, color='springgreen', linestyle='--')

specific_time = pd.Timestamp('2024-05-10T13:51:14.0000000')
plt.axvline(x=specific_time, color='purple', linestyle='--', label='Services at max capacity')

plt.legend()
plt.tight_layout()
plt.show()
