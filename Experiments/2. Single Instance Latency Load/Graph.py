import pandas as pd
import matplotlib.pyplot as plt

# Path to the CSV file
file_path = 'Latency/Client0_Latency.csv'

# Read the CSV file
df = pd.read_csv(file_path, sep=';', header=None, names=['timestamp', 'latency'])

# Convert the timestamp column to datetime
df['timestamp'] = pd.to_datetime(df['timestamp'])

# Set the timestamp column as the index
df.set_index('timestamp', inplace=True)

# Resample the data by millisecond and calculate the mean of latency
df_resampled = df.resample('S').mean()  # 'L' stands for millisecond

# Interpolating missing values
df_resampled.interpolate(method='linear', inplace=True)

# Reset index to use timestamp as a column again
df_resampled.reset_index(inplace=True)

# Read timestamps from Scalability.txt
timestamps_path = 'Scalability.txt'
with open(timestamps_path, 'r') as file:
    timestamps = [pd.to_datetime(line.strip()) for line in file.readlines()]

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

    # Adding vertical lines from Scalability.txt
    for specific_time in timestamps:
        plt.axvline(x=specific_time, color='red', linestyle='--', label='Target Time')

    #plt.legend()
    plt.tight_layout()
    plt.show()
else:
    print("All resampled data points are NaN. Check the original data density and time spread.")