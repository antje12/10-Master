import pandas as pd
import matplotlib.pyplot as plt

file_path = 'Latency/Client0_Latency.csv'
df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])
# Convert to datetime
df['timestamp'] = pd.to_datetime(df['timestamp'])
# Set timestamp as index
df.set_index('timestamp', inplace=True)

# Resample the data by second and calculate latency mean
df_resampled = df.resample('S').mean()  # S = sec, L = ms
# Interpolate missing values
df_resampled.interpolate(method='linear', inplace=True)
# Reset index
df_resampled.reset_index(inplace=True)

# Read timestamps
timestamps_path = 'Scalability.txt'
with open(timestamps_path, 'r') as file:
    timestamps = [pd.to_datetime(line.strip()) for line in file.readlines()]

plt.figure(figsize=(10, 5))
plt.plot(df_resampled['timestamp'], df_resampled['latency'], label='Average Latency per Millisecond')
plt.title('Average Latency Evolution Over Time by Millisecond')
plt.xlabel('Time')
plt.ylabel('Latency (ms)')
plt.grid(True)
plt.xticks(rotation=45)

# Add a horizontal line at 150 ms
plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

# Adding vertical lines from Scalability.txt
for specific_time in timestamps:
    plt.axvline(x=specific_time, color='red', linestyle='--', label='Target Time')

plt.legend()
plt.tight_layout()
plt.show()
