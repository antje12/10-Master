import matplotlib.pyplot as plt
import numpy as np
from sklearn.linear_model import LinearRegression
from sklearn.metrics import r2_score

# Data points
x = [1, 2, 3, 4, 5, 6]
y = [50, 56, 72, 75, 77, 80]

# Reshape x for use in sklearn
x_reshaped = np.array(x).reshape(-1, 1)

# Create linear regression model
model = LinearRegression()
model.fit(x_reshaped, y)  # Fit model on the data

# Predict y values
y_pred = model.predict(x_reshaped)

# Calculate R^2 value
r2 = r2_score(y, y_pred)

# Create the plot
plt.figure(figsize=(3, 5))
plt.plot(x, y, marker='o', label='Actual data')  # Plot with markers
plt.plot(x, y_pred, label='Fit line: y={:.2f}x+{:.2f}'.format(model.coef_[0], model.intercept_))  # Plot the linear fit

# Add title and labels
plt.title('System scalability')
plt.xlabel('Service Instances')
plt.ylabel('Max Concurrent Clients')
plt.legend()
plt.grid(True)
plt.xticks(x)  # Set x-ticks to be exactly as your x data
plt.yticks(range(min(y), max(y)+1, 2))  # Y-ticks from min to max with step of 2

# Show R^2 on the plot
plt.figtext(0.15, 0.75, f'R^2: {r2:.2f}', fontsize=10, bbox={"facecolor":"white", "alpha":0.5, "pad":5})

# Show the plot
plt.show()
