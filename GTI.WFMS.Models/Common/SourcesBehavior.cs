using System.Windows.Interactivity;
using DevExpress.Xpf.Editors.DataPager;
using System.Windows;
using System.Collections;
using System.Collections.Specialized;

namespace GTI.WFMS.Models.Common
{
    public class SourcesBehavior : Behavior<DataPager>
    {
        public static readonly DependencyProperty ActualSourceProperty
            = DependencyProperty.Register("ActualSource", typeof(object), typeof(SourcesBehavior), null);
        public static readonly DependencyProperty SourcesProperty
            = DependencyProperty.Register("Sources", typeof(IList), typeof(SourcesBehavior),
            new PropertyMetadata(null, (d, e) => ((SourcesBehavior)d).OnSourcesChanged((IList)e.NewValue)));

        // 뷰모델의 ItemCnt와 바인딩하기위해 DependencyProperty 추가
        public static readonly DependencyProperty ItemCntProperty
            = DependencyProperty.Register("ItemCnt", typeof(int), typeof(SourcesBehavior), null);


        public object ActualSource
        {
            get { return (object)GetValue(ActualSourceProperty); }
            set { SetValue(ActualSourceProperty, value); }
        }
        public IList Sources
        {
            get { return (IList)GetValue(SourcesProperty); }
            set { SetValue(SourcesProperty, value); }
        }
        //데이터의 총건수 - 바인딩된 뷰모델에서 연결됨
        public int ItemCnt
        {
            get { return (int)GetValue(ItemCntProperty); }
            set { SetValue(ItemCntProperty, value); }
        }


        // 뷰의 DataPager 객체
        public DataPager DataPager { get { return AssociatedObject; } }





        #region ============ Behavior DataPager 상속메소드 ============ 

        // Sources 연결 이벤트
        protected override void OnAttached()
        {
            base.OnAttached();
            //DataPager.PageSize = 10;
            DataPager.ItemCount = ItemCnt; //페이징 Item버튼의 갯수
            SubsribeSourcesColletion(Sources);
            SubscribeDataPager();
        }
        protected override void OnDetaching()
        {
            UnsubsribeDataPager();
            UnsubsribeSourcesColletion(Sources);
            base.OnDetaching();
        }
        void OnSourcesChanged(IList oldSources)
        {
            if (DataPager == null) return;
            UpdateActualSrc();
            //UpdateActualSource(DataPager.PageIndex);
            if (Sources != null) DataPager.ItemCount = ItemCnt;
            UnsubsribeSourcesColletion(oldSources);
            SubsribeSourcesColletion(Sources);
            if (ActualSource != null)
            {
                //DataPager.PageIndex = PageIndex;
                //DataPager.PageIndex = Sources.IndexOf(ActualSource);
            }
            SubscribeDataPager();
        }
        #endregion



        #region =========== 내부 이벤트핸들러 ===============
        void UnsubsribeSourcesColletion(IList sources)
        {
            if (sources is INotifyCollectionChanged)
                ((INotifyCollectionChanged)sources).CollectionChanged -= Sources_CollectionChanged;
        }
        void SubsribeSourcesColletion(IList sources)
        {
            UnsubsribeSourcesColletion(sources);
            if (sources is INotifyCollectionChanged)
                ((INotifyCollectionChanged)sources).CollectionChanged += Sources_CollectionChanged;
        }
        void UnsubsribeDataPager()
        {
            if (DataPager == null) return;
            DataPager.PageIndexChanged -= DataPager_PageIndexChanged;
        }
        void SubscribeDataPager()
        {
            if (DataPager == null) return;
            UnsubsribeDataPager();
            DataPager.PageIndexChanged += DataPager_PageIndexChanged;
        }

        void Sources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Sources != null && Sources.Count > 0)
            {
                DataPager.ItemCount = ItemCnt;
                UpdateActualSrc(); //소스변경되면 무조건 그리드변경
            }
            //소스전체처리하는 경우에 초기 그리드데이터 처리하는부분
            //if (ActualSource == null)
            //{
            //    UpdateActualSource(DataPager.PageIndex);
            //}
        }

        #endregion



        #region ========= 내부함수 =============

        // 전체데이터중에 그리드데이터 표시 - 사용안함
        void UpdateActualSource(int index)
        {
            if (Sources != null && Sources.Count > 0)
            {
                if (index < Sources.Count) ActualSource = Sources[index];
                else ActualSource = Sources[0];
            }
        }

        // 서버페이징된 데이터를 그리드데이터 표시
        void UpdateActualSrc()
        {
            if (Sources != null && Sources.Count > 0)
            {
                ActualSource = Sources[0];
            }
        }


        void DataPager_PageIndexChanged(object sender, DataPagerPageIndexChangedEventArgs e)
        {
            //UpdateActualSource(e.NewValue); 전체모드인 경우에 페이지변경시 그리드조회 - 사용안함
        }
        #endregion



    }
}
