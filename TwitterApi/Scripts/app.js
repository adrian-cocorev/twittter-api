function MakeSearch() {

    $('#tweetsAndSentiments li').remove();

    $('#sectionWaiting').show();

    var query = $('#queryTwitterTerm').val();
    var number = $('#queryNumberOfTweets').val();

    console.log(number);
    console.log(query);

    var url = '';
    if (number != '')
        url = '/api/Tweets/searchTweets?searchTerm=' + query + '&numberOfTweets=' + number;
    else
        url = '/api/Tweets/searchTweets?searchTerm=' + query;

    $.ajax({
        type: 'GET',
        url: url
    }).done(function (data) {
        console.log(data);

        $('#sectionWaiting').hide();

        $('#tweetsAndSentiments li').empty();

        if (data.length === 0)
            $('#tweetsAndSentiments').append('<li><b>Nothing have been found. I\'m sorry!!</b></li>');

        jQuery.each(data, function (i, val) {
            $('#tweetsAndSentiments').append('<li> <b>Username:</b> ' + val.TweetDetails.UserName +
                '<br/><b>Have said:</b> ' + val.TweetDetails.Text +
                '<br/><b>On date:</b> ' + val.TweetDetails.Created +
                '<br/><b>Url:</b> ' + '<a href="' + val.TweetDetails.Url + '" target="_blank">click to go to source</a>' +
                '<div id="chart_div_' + i + '"></div>' +
                '</li>' +
                '<div class="devider"></div>');
            initializeChart(val.TweetPositive, val.TweetNegative, i);
            console.log(val);
        });

    }).fail(function (data) {
        console.log('eroare ' + data);
        $('#sectionWaiting').hide();
        $('#tweetsAndSentiments').append('<li><b>There was an error. I\'m sorry!!</b></li>');
    });
}

function drawSentimentChart(pos, neg, chart_i) {   

    var data = google.visualization.arrayToDataTable([
        ['Tweet', 'Positive', 'Negative'],
        ['', pos, neg]
    ]);

    var options = {
        title: 'Sentiment classification',
        chartArea: { width: '50%', height: '20%'},
        hAxis: {
            title: '',
            minValue: 0
        },
        vAxis: {
            title: ''
        }
    };

    var chart = new google.visualization.BarChart(document.getElementById('chart_div_' + chart_i));
    chart.draw(data, options);
}

function initializeChart(pos, neg, chart_i) {
    drawSentimentChart(pos, neg, chart_i);
}

google.charts.setOnLoadCallback(initializeChart);
google.charts.load('current', { packages: ['corechart', 'bar'] });