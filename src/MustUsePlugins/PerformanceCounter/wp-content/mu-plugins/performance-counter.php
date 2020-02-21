<?php
class YRPNETWORKS_Performance_viewer {
    private $default = array( 'enable' => 1, 'capability' => 'manage_options' );
    private $performance_threshold = array(
        'timer' => array( 0.5, 0.7 ),
        //'query' => array( 50, 100 ),
        'query' => array( 150, 200 ),
    );
    
    public function __construct() {
        add_action( 'admin_bar_menu' , array ( $this, 'performance_information' ), 9999 );
        //add_action( 'wp_enqueue_scripts'   , array( $this, 'enqueue' ) );
		add_action( 'admin_enqueue_scripts', array( $this, 'enqueue' ) );
    }
    
    public function enqueue() {
		wp_enqueue_style( 'yrpnetworks-performance', '/wp-content/mu-plugins/performance/css/performance-admin-bar.css', array(), "1.0.0" );
	}


	public function performance_information( $wp_admin_bar ) {
		$timer = timer_stop();
		$queries = get_num_queries();

		if ( $timer > $this->performance_threshold['timer'][1] || $queries > $this->performance_threshold['query'][1] ) {
			$class = 'bad';
		} elseif ( $timer > $this->performance_threshold['timer'][0] || $queries > $this->performance_threshold['query'][0] ) {
			$class = 'attention';
		} else {
			$class = 'good';
		}

		$wp_admin_bar->add_menu(array(
			'id'     => 'wp-performances',
			'parent' => 'top-secondary',
			'meta'   => array( 'class' => $class ),
			'title'  => '<span class="ab-icon"></span><span class="ab-label">' . esc_html( $timer ) . ' sec. ' . esc_html( $queries ) . " queries.</span>\n",
			'href'   => false,
		));
	}
}

new YRPNETWORKS_Performance_viewer;