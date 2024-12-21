# Use the official Node.js 20 image from the Docker Hub (Debian-based)
FROM node:20

# Install necessary system dependencies for canvas and fonts
RUN apt-get update && apt-get install -y \
    build-essential \
    libcairo2-dev \
    libpango1.0-dev \
    libjpeg-dev \
    libgif-dev \
    librsvg2-dev \
    libtool \
    autoconf \
    automake \
    fontconfig \ 
 && rm -rf /var/lib/apt/lists/*

# Set the working directory inside the container
WORKDIR /usr/src/app

# Copy package.json and yarn.lock to the container
COPY package.json yarn.lock ./

# Install dependencies using Yarn
RUN yarn install

# Rebuild canvas native bindings (if required)
RUN yarn add canvas@2.10.2

# Copy the rest of your application code
COPY . .

# Install custom fonts(e.g., DancingScript)
COPY DancingScript-Regular.ttf /usr/share/fonts/truetype/custom/
RUN fc-cache -f -v

# Check if the fonts are installed
RUN fc-list | grep "DancingScript"

# Expose the port your app runs on
EXPOSE 7154

# Command to run your app
CMD ["node", "app.js"]
