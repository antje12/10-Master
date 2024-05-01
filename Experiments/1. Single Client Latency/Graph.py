import pandas as pd
import matplotlib.pyplot as plt

# Path to the CSV file
file_path = 'Latency.csv'

# Read the CSV file
df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])

# Convert the timestamp column to datetime
df['timestamp'] = pd.to_datetime(df['timestamp'])

# Set the timestamp column as the index
df.set_index('timestamp', inplace=True)

# Check the spread of data points
print("Timestamp frequency stats:")
print(df.index.to_series().diff().describe())

# Resample the data by millisecond and calculate the mean of latency
df_resampled = df.resample('S').mean()  # 'L' stands for millisecond

# Interpolating missing values
df_resampled.interpolate(method='linear', inplace=True)

# Reset index to use timestamp as a column again
df_resampled.reset_index(inplace=True)

# Plotting the data
plt.figure(figsize=(10, 5))
if not df_resampled['latency'].isna().all():  # Check if all values are NaN
    plt.plot(df_resampled['timestamp'], df_resampled['latency'], label='Average Latency per Millisecond')
    plt.title('Average Latency Evolution Over Time by Millisecond')
    plt.xlabel('Time')
    plt.ylabel('Latency (ms)')
    plt.grid(True)
    plt.xticks(rotation=45)

    # Add a horizontal line at 150 ms
    plt.axhline(y=150, color='blue', linestyle='--', label='Threshold at 150 ms')

    plt.legend()
    plt.tight_layout()
    plt.show()
else:
    print("All resampled data points are NaN. Check the original data density and time spread.")
